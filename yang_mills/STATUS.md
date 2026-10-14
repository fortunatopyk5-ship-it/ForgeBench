# STATUS

Current objective completed: adversarial audit of CMP122-II (1.91) upstream -> regular R' composition. No K_0, STEP_0, boundary B' or new RG sectors.

Current rigorous verdict:
- UPSTREAM C^1: GAP.
- COMPLEX BALL: GAP.
- OUTPUT IMAGE STRICTLY INSIDE R' BALL: NO (whole-ball inclusion not proved).
- COMPOSED C^1: GAP.
- UNIFORMITY IN Sigma: UNKNOWN for independent coefficient perturbations.

The Iteration-10 PROVED labels for YM-RPRIME-191-C1-1, YM-V-PRODUCER-BOUND-1, YM-RPRIME-UPSTREAM-C1-1 and YM-RPRIME-UPSTREAM-COMPOSED-C1-1 are withdrawn as source-instantiated theorems. Conditional abstract implications remain valid.

Accepted unchanged:
F -> native regular R'_Bal is bounded C^1 on the audited anchored ball with
C_inc=9, c_stitch=sqrt(7),
r_*=(a/9)exp[-16a-sqrt(7)theta].
The stronger downstream Schur result remains conditional on its explicit decay reserve.

Surviving source-family norm certificate:
M_F^src(X')=exp[-p_0(g_k)]exp[-(1+beta)kappa d_k(X')],
mu_F MUST equal downstream mu_in=(1+beta)kappa-lambda_in,
C_geo(lambda_in)<infinity.
Therefore ||F^src||_F<=C_geo(lambda_in)exp[-p_0(gamma)], which can be made <r_*. This does not prove image inclusion for an entire complex coefficient ball.

First failed inference:
strict slack in physical source estimates was taken to imply normal convergence and the same decay for arbitrary independently perturbed T' operators and V coefficients. The source T bounds use positive integral density, controlled complex exponent and history/geometric restrictions. The old chart has not been shown to preserve all inequalities of (1.92)--(1.97).

Exact new blockers:
1. YM-191-COMPLEX-MAJORANT-1: explicit compatible operator domains and uniform anchored absolute majorant for all independent (T',V) on radii >1, with the SAME downstream exponent.
2. V producer source instantiation: explicit restriction/localization kernel L_{Sigma,Y,lambda} and uniform operator row estimate over volume, cutoff, k and Sigma. The abstract mixed-norm row implication is correct; its claimed application was not established.
3. YM-UPSTREAM-IMAGE-RESERVE-1: prove R_up>1 and r_image<r_* uniformly on the whole complex input ball, tracking producer normalization constants in the nonlinear majorant.

Only after these are proved may DH_R'(U(K)) o DU(K) be used with the correct radii. See rprime_upstream_c1.md for the conditional constants, source inequality audit, formal support statements and necessary V=0 check.

The intended chart remains (T',E_ren,R_ren,B_loc,K_aux) over frozen Sigma. E_raw relevant extraction is a separate earlier chart problem, not the new blocker. Hard characteristics are not differentiated.

The unrelated earlier crossover records below are historical and outside this audit; their suggested STEP_0 work was not undertaken.

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
