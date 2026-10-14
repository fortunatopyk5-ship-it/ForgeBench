# STATUS

Current objective:
Iteration 10 is restricted to the upstream CMP122-II Eq. (1.91) map that constructs the regular pre-exponentiation activity F(X'). The audited downstream F -> R'_Bal theorem is accepted and not reopened. Boundary B', source marks, K_0 and STEP_0 are not attacked.

Current strongest new result:
Eq. (1.91) itself has been separated from the earlier localization machinery.

DIRECT PRIMARY TRANSCRIPTION:
For fixed hard history/domain data Sigma,

F(X')
=
sum_q
sum'_{ {X_{j_1},...,X_{j_q}} }
sum'_D
[
 product_{Y in D} int_0^1 dt(Y)
]
[
 product_{h=1}^q T'_k(X_{j_h})
]
[
 product_{Y in D}V(Y)
]
exp[
 sum_{Y in D}t(Y)V(Y)
].

The primed sums contain exactly the frozen localization-domain data whose special connected component is X'.

Thus Eq. (1.91) directly depends only on two ALREADY-LOCALIZED coefficient families:
T'_k(X) and V(Y).
Raw E/R/B coordinates have already been absorbed before this formula.

Factorization:
raw E/R/B/T/history coefficients
 --P_Sigma-->
(T',V)
 --M_1.91-->
F
 --H_R'-->
R'_Bal.

Verified this iteration:
- hard determining/history labels are frozen base data, not Banach tangent coordinates;
- within one fixed Sigma, the index condition "determines exactly X'" is coefficient-independent;
- the Eq. (1.91) elementary maps are restriction, multilinear products, exponential, bounded t-integration, local T-operator products and normally convergent resummation;
- exact termwise first derivative formulas are recorded in YM-RPRIME-191-ELEMENTARY-DERIV-1;
- local normalized T integration is C^1 with
  D barT[h]F=barT(hF)-barT(h)barT(F);
- a mixed source-majorant + anchored input topology is required; anchored norm alone is too weak to preserve the pointwise source majorants;
- after choosing the source small-coupling threshold with strict slack, there is a cutoff/volume/depth-independent open coefficient radius R_src>1;
- the absolute proof of (1.92)-(1.97) supplies normal convergence on that coefficient ball;
- therefore
  M_1.91:(T',V)->F
  is Banach-holomorphic and uniformly C^1;
- on the inner unit ball,
  ||D M_1.91|| <= C_1.91 = rho_F/(R_src-1);
- exact reduced Jacobian support:
  D_eta F(X')=0 unless eta subset X';
  hence reduced collar r_col=0;
- composition with the accepted downstream theorem gives
  ||D(H_R' o M_1.91)|| <= C_R' C_1.91
  for the reduced coordinates (T',V).

Important limitation:
The desired RAW theorem
(T,E,R,B,local history coefficients)->F
is NOT yet proved.

FIRST FAILED SUBMAP:
the producer

P_V:
(K_E,K_R,K_B,K_old,...)
->
(V(Y))_Y.

CMP122-II pp.376-377 describe this producer as localization + E/R renormalization + resummation + addition/subtraction before establishing (1.68),(1.69). The source proves physical-field analyticity and output bounds, but a bounded difference/Frechet theorem for P_V in a fixed anchored coefficient Banach space has not yet been extracted.

The local exponential/integration map producing T' from an already-localized exponent insertion is NOT the blocker; it is C^1. However the same unresolved raw-to-local-potential producer feeds the V(X~) term inside T'.

Schur status for the upstream map:
- bounded anchored/mixed operator norm: PROVED by full Banach Cauchy;
- exact support eta subset X': PROVED;
- stronger quantitative off-diagonal Schur kernel: OPEN, because a marked two-anchor version of the source (1.92)-(1.97) summation is still needed. Exact support alone does not sum the mixed coordinate regulator.

Current theorem status:
YM-RPRIME-191-C1-1:
PROVED for already-localized (T',V) coordinates under strict source-majorant slack.

YM-RPRIME-UPSTREAM-C1-1:
PARTIAL / BLOCKED AT YM-RPRIME-UPSTREAM-PRODUCER-1.

Current blocker:
extract the coefficient-space localization/renormalization/resummation operator producing V(Y), including its exact support kernel and anchored operator norm. Do not redo Eq. (1.91) itself.

Next action:
In a future run, trace the pp.376-377 V(Y) producer back into CMP119 Sect. 2 and isolate the E/R localization-renormalization maps as explicit bounded linear/analytic coefficient operators. This run stops before that next source extraction.

Iteration-10 verdict:
STATUS: EQ. (1.91) REDUCED MAP (T',V)->F BOUNDED C1 PROVED; RAW (T,E,R,B,...)->F BLOCKED AT THE V-PRODUCER.


## Update — Iteration 2 (YM-CROSSOVER-1 only)

STATUS:
YM-CROSSOVER-1 REDUCED TO EXPLICIT SUBLEMMAS; inclusion itself NOT proved.

Strongest new result:
The measure-only claim
R^M(K_match) subset D_KP
is insufficient for the original microscopic mass gap unless source/observable transport is controlled. The corrected crossover package is:

(A) YM-MATCH-EXTRACT-1:
extract a global, cutoff/volume-uniform K_match from the UV construction in the concrete Banach space of crossover_spec.md.

(B) YM-TUBE-1 + YM-RG-DRIFT-1:
prove finite one-step enclosures for beta and the full remainder norm. Once a uniform drift delta>0 and invariant remainder radius R exist, M_G<infinity follows and is no longer an independent heuristic assumption.

(C) YM-RG-SOURCE-1:
control source transport / conditional covariance across eliminated RG shells.

(D) Final KP landing:
Q_{alpha,mu}(z_Phi)<alpha. YM-KP-ANCHOR-1 then gives uniform cluster expansion and exponential coarse correlations.

Concrete Banach space:
B_{alpha,mu,p,rho} of globally defined gauge-invariant polymer interactions Phi_X on connected 4D block polymers, with norm
sup_B sum_{X contains B} exp(alpha|X|+mu diam(X)) ||Phi_X||_{C^p,rho}.
Full definition: crossover_spec.md.

Corrected blocker:
The hardest unresolved estimate is no longer the abstract statement "M_G finite". It is the construction of rigorous one-step Yang-Mills enclosures

F_j^-(beta,r) <= beta(R Phi) <= F_j^+(beta,r),
r(R Phi) <= G_j(beta,r)

on an invariant tube, together with source-locality estimates, starting from a rigorously extracted K_match.

Verified this iteration:
- scalar-coupling-only flow is insufficient (explicit toy counterexamples);
- q<alpha in the anchored weighted polymer norm is a sufficient KP condition;
- the finite invariant-tube induction is rigorous;
- a positive uniform beta drift plus invariant remainder tube implies cutoff-independent finite M;
- previous cutoff bridge can be strengthened to positive-time correlator convergence only (YM-CUTOFF-BRIDGE-2), so divergent equal-time renormalization constants/prefactors need not be assumed controlled.

Validated-numerics support:
scripts/verify_crossover_tube.py uses exact rational interval arithmetic to certify the finite tube induction once rigorous one-step bounds are supplied. The bundled JSON is explicitly only a toy example.


### Final refinement — Iteration 2

The one-step invariant-tube problem has now been reduced further by YM-RG-DERIV-1 and YM-RG-TAYLOR-1.

For Phi=beta W+eta with ell(eta)=0 and ||eta||<=r, define
f(beta)=ell(R(beta W)),
h(beta)=||Q R(beta W)||.
A rigorous one-step tube enclosure follows from five scalar bound families on each compact beta interval I_j:

1. center flow: f_j^- <= f(beta) <= f_j^+;
2. generated center remainder: h(beta) <= H_j;
3. relevant/irrelevant mixing: ||ell D R_{beta W} Q|| <= A_j;
4. irrelevant amplification: ||Q D R_{beta W} Q|| <= B_j;
5. curvature: sup_{Phi in K_j} ||D^2 R_Phi|| <= C_j.

It then suffices that

beta_{j+1}^- <= f_j^- - A_j R_j - (1/2)||ell|| C_j R_j^2,

beta_{j+1}^+ >= f_j^+ + A_j R_j + (1/2)||ell|| C_j R_j^2,

R_{j+1} >= H_j + B_j R_j + (1/2)||Q|| C_j R_j^2.

For the exact block-integration part of R,
D E_Phi[A]=E_{Phi,V}[A] and
D^2 E_Phi[A,B]=-Cov_{Phi,V}(A,B).
Thus the fifth bound is concretely a one-shell conditional-covariance estimate, not an abstract Frechet-regularity placeholder.

Current deepest blocker:
obtain rigorous, volume-independent Yang-Mills bounds for f,H,A,B,C on at least the first intermediate-coupling interval emerging from YM-MATCH-EXTRACT-1. This is the exact next step.


### Adversarial corrections — final

1. D_KP is NOT equivalent to a mass gap. It is only a sufficient landing chart; even exactly product measures can violate a badly chosen product-Haar activity smallness condition if local weights are large. Therefore failure of YM-CROSSOVER-1 in this chart would not imply gaplessness.

2. Polymer incompatibility must be defined using the actual collar/link support of Phi_X, not just the core block set.

3. Source control must start before the finite crossover: the UV-to-matching package must deliver quasi-local source kernels for microscopic gauge-invariant observables (YM-UV-SOURCE-MATCH-1).

4. The practical finite-drift lemma is YM-RG-DRIFT-2, with a bounded terminal beta strip and a no-overshoot estimate.

Final status of Iteration 2:
STATUS: YM-CROSSOVER-1 REDUCED TO EXPLICIT LEMMAS/INEQUALITIES — NOT PROVED.

For the measure inclusion itself, the remaining Yang-Mills content is:
- MATCH: rigorous K_match extraction in the concrete global norm;
- STEP_j: rigorous f_j^±, H_j, A_j, B_j, C_j bounds for finitely many intermediate-coupling intervals;
- LAND: final Q_{alpha,mu}<alpha estimate.

For use in the mass-gap chain, additionally:
- UV-SOURCE: microscopic-to-matching source localization;
- CROSSOVER-SOURCE: shell-by-shell source/conditional-covariance bounds.

Exact next attack:
prove or falsify the FIRST one-step certificate STEP_0 in the chosen norm, starting from the strongest matching estimates that can actually be extracted from Balaban's small-field/large-field RG results.
