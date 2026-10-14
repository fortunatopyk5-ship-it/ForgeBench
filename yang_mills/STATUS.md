# STATUS

Current objective:
Iteration 10 is restricted to CMP122-II Eq. (1.91) and the source-native upstream coefficient map feeding the already-audited regular R' cluster/log theorem. Downstream F->R'_Bal is accepted. Boundary B', physical source insertion, K_0 and STEP_0 are not attacked.

Current strongest result:
The regular source-native chain

(T', E_ren, R_ren, B_loc, local frozen history data)
   -> F_reg
   -> R'_Bal

is uniformly bounded C^1 on a cutoff/volume/RG-depth-independent anchored coefficient neighborhood.

Exact Eq. (1.91):
For one frozen history/domain sector Sigma,

F(X')
=
sum_{q>=0}
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

For regular X' disjoint from the fixed current large-field domains, the source p.389 says q starts at 1.

Functional factorization:
source-native current coefficients
 --P_Sigma-->
(T',V)
 --M_1.91^reg-->
F_reg
 --H_R'-->
R'_Bal.

History/domain labels are frozen base data; no derivative of hard characteristic thresholds is taken.

SOURCE-NATIVE INPUT CHART:
CMP119 explicitly states:
- E_k is already fully renormalized, with vacuum and coupling counterterms included;
- R_k is already vacuum-renormalized;
- B_k requires no renormalization.

Therefore the natural upstream Banach state uses E_ren, R_ren, B_loc, not a pre-renormalization E_raw coefficient.

The lower-level chart transition E_raw->E_ren is a separate problem and is not silently included in this theorem.

Reduced Eq. (1.91) theorem:
YM-RPRIME-191-C1-1 is PROVED.

On a frozen history fiber, use a mixed input norm:
- weighted source-majorant sup regulator;
- anchored exponential l1 term.

After shrinking the fixed source coupling threshold with strict slack, there exists R_src>1, independent of volume/cutoff/k, such that the regular map

M_1.91^reg:(T',V)->F_reg

is Banach-holomorphic on the R_src-ball.

On the source unit ball,

||D M_1.91^reg||
<=
C_1.91
=
rho_F/(R_src-1),

where rho_F<r_*^native is the inflated source-majorant F-radius.

Exact reduced support:
D_eta F(X')=0 unless eta subset X'.

Source-native V producer:
YM-V-PRODUCER-BOUND-1 is PROVED.

After the local E/R/B atoms are taken in the CURRENT CMP119 renormalized coordinates, the pp.376-377 V(Y) construction is a signed linear incidence resummation. The direct source proof is an absolute majorant proof and supplies a uniform row bound of the form

sum_lambda |A_Sigma(Y,lambda)| M_lambda
<=
C_res M_V(Y).

After harmless normalization,

||P_V||<=1

in the mixed source-majorant/anchored norms.

Terms with localization domain equal to a whole large-field component satisfy the worse (1.69), but CMP122-II immediately after (1.72) says those terms are included into T'_k and omitted from the V exponential. Hence the V-sector entering Eq. (1.91) is the decaying (1.68) class; no boundary B' theorem is used.

Full source-native upstream:
YM-RPRIME-UPSTREAM-C1-1 is PROVED.

With

P_Sigma(T',E_ren,R_ren,B_loc,K_aux)
=
(T',P_V(...))

and normalized ||P_Sigma||<=1,

U_Sigma
=
M_1.91^reg o P_Sigma

is uniformly C^1 and

sup ||D U_Sigma||
<=
C_U
<=
C_1.91.

Raw/source-native pointwise Cauchy:
for input coordinate lambda in sector s,

||D_lambda U_Sigma(K)_{X'}[h]||
<=
(A_F/delta_up)
w_up(lambda)
e^{-kappa_F d_k(X')}
||h||,

with

w_up(lambda)
=
max{
 M_s(lambda)^(-1),
 C_s^(-1)e^{mu_s d(X_lambda)}
}.

Exact raw support:
- T' coordinate eta can affect F(X') only if eta subset X';
- E_ren/R_ren/B_loc/auxiliary lambda can affect F(X') only if there exists
  Y in Out_Sigma(lambda)
  with Y subset X'.

Downstream composition:
YM-RPRIME-UPSTREAM-COMPOSED-C1-1 is PROVED:

||D(R'_Bal o U_Sigma)||
<=
C_R' C_U
<=
C_R' C_1.91.

Important remaining limitation:
A quantitative OFF-DIAGONAL Schur kernel for the upstream producer is NOT proved. The exact frozen incidence support is known, but a marked two-anchor estimate for A_Sigma(Y,lambda) is still needed to sum the mixed coordinate regulator geometrically.

Lower-level chart issue:
YM-E-RELEVANT-EXTRACT-1 remains OPEN only for a pre-renormalization E_raw chart. CMP119 (3.64) identifies the relevant coefficient via a second-order kernel extraction. This is not required for the current source-native inductive-state theorem, where E_ren is already the state coordinate.

Verified direct-primary source facts this iteration:
- CMP122-II pp.387-389: exact (1.91) formula, frozen determining condition, regular q>=1 case, and absolute majorant (1.92)-(1.97).
- CMP122-II pp.376-377: V(Y) is obtained by linear resummations/localizations after renormalized local atoms are supplied; the proof gives (1.68)-(1.69).
- CMP122-II after (1.72): whole-component V terms are included into T'_k, not the V exponential.
- CMP122-II (1.71),(1.74),(1.75),(1.89): local T operation, nonzero normalization, normalized operator bound, and the explicit extra -kappa d_k(X) tree factor.
- CMP119 pp.259-260: E fully renormalized; R vacuum-renormalized; B unrenormalized/already localized.
- CMP119 p.283: relevant E extraction/counterterm structure (3.64)-(3.67), used only to identify the lower-level chart issue.

Current theorem status:
YM-RPRIME-191-C1-1: PROVED.
YM-V-PRODUCER-BOUND-1: PROVED in source-native renormalized coordinates.
YM-RPRIME-UPSTREAM-C1-1: PROVED in source-native renormalized coordinates.
YM-RPRIME-UPSTREAM-COMPOSED-C1-1: PROVED.
Upstream off-diagonal Schur: OPEN.
Lower-level E_raw->E_ren chart transition: OPEN / OUTSIDE CURRENT UPSTREAM THEOREM.

Not attacked:
- boundary B' lane;
- full outer CMP119 history map;
- physical source insertion;
- D^2;
- K_0;
- STEP_0.

Iteration-10 verdict:
STATUS: SOURCE-NATIVE CMP122-II (1.91) UPSTREAM MAP BOUNDED C^1 PROVED; COMPOSED REGULAR R' MAP BOUNDED C^1 PROVED. STRONGER UPSTREAM SCHUR REMAINS OPEN.


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
